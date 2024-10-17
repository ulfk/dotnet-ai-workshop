# Vector Search (Linux version)

Continued from [VectorSearch.md](./3_VectorSearch.md). Read that first.

You're going to use FAISS to build an index of many thousands of GitHub issue titles posted to the `dotnet/runtime` repo, and implement semantic search.

## Open the project

Open the project `exercises/Embeddings/Begin`.

In `Program.cs`, ensure only `FaissSemanticSearch` is uncommented:

```cs
//await new SentenceSimilarity().RunAsync();
//await new ManualSemanticSearch().RunAsync();
await new FaissSemanticSearch().RunAsync();
```

In `FaissSemanticSearch.cs`, in `RunAsync`, add a method call that will build the index:

```cs
var index = await LoadOrCreateIndexAsync(githubIssues);
```

## Building an index

Start implementing `LoadOrCreateIndexAsync` as follows:

```cs
private async Task<FaissMask.IndexIDMap> LoadOrCreateIndexAsync(IDictionary<int, GitHubIssue> data)
{
    var index = new FaissMask.IndexIDMap(new FaissMask.IndexFlat(EmbeddingDimension));
}
```

This will set up a flat index. So it's very similar to what you built manually in the previous exercise.

Continue by populating and returning it:

```cs
foreach (var issuesChunk in data.Chunk(1000))
{
    Console.Write($"Embedding issues: {issuesChunk.First().Key} - {issuesChunk.Last().Key}");
    var embeddings = await EmbeddingGenerator.GenerateAsync(issuesChunk.Select(i => i.Value.Title));
    Console.WriteLine(" Inserting into index...");

    index.Add(
        embeddings.Select(e => e.Vector.ToArray()).ToArray(),
        issuesChunk.Select(i => (long)i.Key).ToArray());
}

return index;
```

This asks `EmbeddingGenerator` for embeddings in groups of 1000. You could do it all at once, but most hosted services will have limits.

Try running it. Hopefully in 10 seconds or so, it will compute and index the embeddings for 10,000 issue titles.

### Sidenote: `LocalEmbeddingsGenerator`

Unlike the previous exercise, this does *not* use Ollama (or even OpenAI) to generate the embeddings. It's running an embedding model locally on your CPU, using the [ONNX runtime](https://onnxruntime.ai/). This is generally much faster than calling out to Ollama or OpenAI if you're only working with short strings. But in production, OpenAI or a similar service still has the advantage that it's not blocking your CPU with heavy work.

## Semantic search

Let's search that index. Back in `RunAsync`, add a search loop:

```cs
while (true)
{
    Console.Write("\nQuery: ");
    var input = Console.ReadLine()!;
    if (input == "") break;

    var inputEmbedding = (await EmbeddingGenerator.GenerateAsync(input))[0];
    var closest = index.Search(inputEmbedding.Vector.ToArray(), 3).ToList();
    foreach (var result in closest)
    {
        var id = (int)result.Label;
        var distance = result.Distance;
        Console.WriteLine($"({distance:F2}): {githubIssues[id].Title}");
    }
}
```

Try it out! You can search for anything that might appear in dotnet/runtime issue titles, for example:

 * `is .net fast`
 * `why doesn't memory get reclaimed`
 * `please stop making Blazor so amazing, it's embarassing the rest of the ecosystem`

You should get relevant matches even when different wording or spellings are used.
