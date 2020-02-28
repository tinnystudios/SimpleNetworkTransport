namespace SimpleTransport
{
    public struct ReaderHeader
    {
        public bool Matched;
        public int ConnectionId;
        public int InstanceId;

        public ReaderHeader(bool matched)
        {
            Matched = false;
            ConnectionId = -1;
            InstanceId = -1;
        }
    }
}