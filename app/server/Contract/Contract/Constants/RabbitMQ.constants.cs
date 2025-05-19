namespace Contract.Constants;

public static class RabbitMQConstant
{
    public static class QUEUE
    {
        public static class NAME
        {
            // ======================= Identity ============================
            // ======================= User ============================
            public const string DELETE_USER = "delete-user-queue";
            public const string RESTORE_USER = "restore-user-queue";
            // ======================= Task ============================
            // ======================= Other ============================
        }
    }
    public static class EXCHANGE
    {
        public static class NAME
        {
            // ======================= Identity ============================
            // ======================= USER ============================
            public const string DELETE_USER = "delete-user-event";
            public const string RESTORE_USER = "restore-user-event";
            // ======================= Task ============================
            // ======================= Other ============================
        }

        public static class TYPE
        {
            /// <summary>
            /// Exchange type used for AMQP direct exchanges.
            /// </summary>
            public const string Direct = "direct";

            /// <summary>
            /// Exchange type used for AMQP fanout exchanges.
            /// </summary>
            public const string Fanout = "fanout";

            /// <summary>
            /// Exchange type used for AMQP headers exchanges.
            /// </summary>
            public const string Headers = "headers";

            /// <summary>
            /// Exchange type used for AMQP topic exchanges.
            /// </summary>
            public const string Topic = "topic";

            private static readonly string[] s_all = { Fanout, Direct, Topic, Headers };

            /// <summary>
            /// Retrieve a collection containing all standard exchange types.
            /// </summary>
            public static ICollection<string> All()
            {
                return s_all;
            }
        }
    }
}
