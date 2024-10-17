# Vector Search

In the preceding exercise, you created a flat list of embedding vectors in memory, and implemented nearest-neighbour search using a simple linear scan.

This already scales up more than you might think. There are ways of shrinking embedding vectors down to just a few tens of bytes each, and then a vectorized linear search can find the closest from 100,000 candidates in under 3ms on a laptop CPU ([example implementation](https://github.com/dotnet/smartcomponents/blob/main/docs/local-embeddings.md#performance)).

### Scaling up

Ultimately the time for a flat scan grows linearly with the number of candidates. If you want to search through millions, billions, or more candidates at high frequency, you'll need bigger machinery. The two most common options are:

 * **Vector databases**. There are dedicated (Qdrant, Milvus, etc.), plus many other databases have added vector indexing (Postgres, CosmosDB, etc.). These set up indexes tuned to find nearest neighbours in sublinear time.
 * [**FAISS**](https://github.com/facebookresearch/faiss) is a very widely-used standalone C/C++ library that implements many advanced vector indexes. In fact, many of the above databases use it for vector search. You can use it directly from .NET, Python, etc.

For realistic production apps, you'd use a vector database, maybe hosted in Docker/Azure Container Apps/etc. But in this exercise you'll use FAISS directly, so that you can appreciate some of the underlying power and configurability.

**CAUTION**: It's surprisingly difficult to get FAISS to actually run, because they don't distribute official binaries, and there isn't a single NuGet package that works cross-platform. So:

 * For Windows, we'll use [FaissSharp](https://www.nuget.org/packages/FaissSharp)
 * For Linux, we'll use [FaissMask](https://www.nuget.org/packages/FaissMask/)
 * If you're on macOS, sorry! FaissMask includes macOS native dependencies, but they never worked when I tried it. Maybe it will on your machine but I doubt it. Please pair-program with someone on Windows or Linux.

If you have the choice, use Windows for this exercise because FaissSharp has more capabilities and is more interesting.

## The path splits here

 * For Windows, continue in [3_VectorSearch_Windows.md](./3_VectorSearch_Windows.md)
 * For Linux, continue in [3_VectorSearch_Linux.md](./3_VectorSearch_Linux.md)
