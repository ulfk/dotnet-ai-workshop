using System.Numerics.Tensors;
using Microsoft.Extensions.AI;

namespace Embeddings;

public class ManualSemanticSearch
{
    public async Task RunAsync()
    {
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator =
            new OllamaEmbeddingGenerator(new Uri("http://127.0.0.1:11434"), modelId: "all-minilm");

        // Create embeddings for all the test data
        var embeddingsResult = await embeddingGenerator.GenerateAsync(TestData.DocumentTitles.Values);
        Console.WriteLine($"Computed {embeddingsResult.Count} embeddings");

        // Can also be done with .Zip, but is less obvious
        var docInfoWithEmbeddings = TestData.DocumentTitles.Select((docTitle, index) => new
        {
            Id = docTitle.Key,
            Text = docTitle.Value,
            Embedding = embeddingsResult[index].Vector,
        }).ToList();

        while (true)
        {
            Console.Write("\nQuery: ");
            var input = Console.ReadLine()!;
            if (input == "") break;

            var inputEmbedding = (await embeddingGenerator.GenerateAsync(input))[0];
            var closest =
                from candidate in docInfoWithEmbeddings
                let similarity = TensorPrimitives.CosineSimilarity(
                    candidate.Embedding.Span, inputEmbedding.Vector.Span)
                orderby similarity descending
                select new { candidate.Text, Similarity = similarity };

            foreach (var result in closest.Take(3))
            {
                Console.WriteLine($"({result.Similarity:F2}): {result.Text}");
            }
        }
    }
}
