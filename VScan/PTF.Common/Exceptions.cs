/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;

namespace PremierTaxFree
{
    public class MessageException : Exception
    {
        public MessageException()
            : base()
        {
        }
        public MessageException(string message)
            : base(message)
        {
        }
        public MessageException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    public class AppInfoException : MessageException
    {
        public AppInfoException(string message)
            : base(message)
        {
        }
    }

    public class AppWarningException : MessageException
    {
        public AppWarningException(string message)
            : base(message)
        {
        }
    }

    public class AppExclamationException : MessageException
    {
        public AppExclamationException()
            : base()
        {
        }
        public AppExclamationException(string message)
            : base(message)
        {
        }
        public AppExclamationException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }

    public class AppStopException : Exception
    {
        public AppStopException(string message)
            : base(message)
        {
        }
    }

    public class NotAllowedToolException : AppExclamationException
    {
        public NotAllowedToolException()
            : base("This tool is not allowed by your administrator")
        {
        }

        public NotAllowedToolException(string message)
            : base(message)
        {
        }
    }

    public class NoImageFoundException : AppExclamationException
    {
        public NoImageFoundException()
            :base("No image found")
        {
        }
    }

    public class QueueIsEmptyException : AppExclamationException
    {
        public QueueIsEmptyException()
            : base("No ids found")
        {
        }
    }
}
