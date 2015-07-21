using System.Threading;
using System.Threading.Tasks;
using Amazon.Kinesis;
using Amazon.Kinesis.Model;

namespace log4net.Ext.Appender
{
    public class NoOpAmazonKinesisClient : IAmazonKinesis
    {
        public void Dispose()
        {

        }

        public AddTagsToStreamResponse AddTagsToStream(AddTagsToStreamRequest request)
        {
            return null;
        }

        public Task<AddTagsToStreamResponse> AddTagsToStreamAsync(AddTagsToStreamRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<AddTagsToStreamResponse>(() => null);
        }

        public CreateStreamResponse CreateStream(CreateStreamRequest request)
        {
            return null;
        }

        public Task<CreateStreamResponse> CreateStreamAsync(CreateStreamRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<CreateStreamResponse>(() => null);
        }

        public DeleteStreamResponse DeleteStream(DeleteStreamRequest request)
        {
            return null;
        }

        public Task<DeleteStreamResponse> DeleteStreamAsync(DeleteStreamRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<DeleteStreamResponse>(() => null);
        }

        public DescribeStreamResponse DescribeStream(DescribeStreamRequest request)
        {
            return null;
        }

        public Task<DescribeStreamResponse> DescribeStreamAsync(DescribeStreamRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<DescribeStreamResponse>(() => null);
        }

        public GetRecordsResponse GetRecords(GetRecordsRequest request)
        {
            return null;
        }

        public Task<GetRecordsResponse> GetRecordsAsync(GetRecordsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<GetRecordsResponse>(() => null);
        }

        public GetShardIteratorResponse GetShardIterator(GetShardIteratorRequest request)
        {
            return null;
        }

        public Task<GetShardIteratorResponse> GetShardIteratorAsync(GetShardIteratorRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<GetShardIteratorResponse>(() => null);
        }

        public ListStreamsResponse ListStreams()
        {
            return null;
        }

        public ListStreamsResponse ListStreams(ListStreamsRequest request)
        {
            return null;
        }

        public Task<ListStreamsResponse> ListStreamsAsync(ListStreamsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<ListStreamsResponse>(() => null);
        }

        public ListTagsForStreamResponse ListTagsForStream(ListTagsForStreamRequest request)
        {
            return null;
        }

        public Task<ListTagsForStreamResponse> ListTagsForStreamAsync(ListTagsForStreamRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<ListTagsForStreamResponse>(() => null);
        }

        public MergeShardsResponse MergeShards(MergeShardsRequest request)
        {
            return null;
        }

        public Task<MergeShardsResponse> MergeShardsAsync(MergeShardsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<MergeShardsResponse>(() => null);
        }

        public PutRecordResponse PutRecord(PutRecordRequest request)
        {
            return null;
        }

        public Task<PutRecordResponse> PutRecordAsync(PutRecordRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<PutRecordResponse>(() => null);
        }

        public PutRecordsResponse PutRecords(PutRecordsRequest request)
        {
            return null;
        }

        public Task<PutRecordsResponse> PutRecordsAsync(PutRecordsRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<PutRecordsResponse>(() => null);
        }

        public RemoveTagsFromStreamResponse RemoveTagsFromStream(RemoveTagsFromStreamRequest request)
        {
            return null;
        }

        public Task<RemoveTagsFromStreamResponse> RemoveTagsFromStreamAsync(RemoveTagsFromStreamRequest request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<RemoveTagsFromStreamResponse>(() => null);
        }

        public SplitShardResponse SplitShard(SplitShardRequest request)
        {
            return null;
        }

        public Task<SplitShardResponse> SplitShardAsync(SplitShardRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return new Task<SplitShardResponse>(() => null);
        }
    }
}
