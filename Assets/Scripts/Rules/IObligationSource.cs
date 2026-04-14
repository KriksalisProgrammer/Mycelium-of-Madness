
    public interface IObligationSource
    {
        string ObligationId { get; }
        bool IsFulfilled { get; }
        void ResetForNewDay();
    }
