using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeOnboard.Application.Shared
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        protected Result(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;

        }
        public static Result Success(string message = "")
            => new Result(true, message);

        public static Result Failure(string message)
            => new Result(false, message);

    }

    public class Result<T> : Result
    {
        public T Data { get; private set; }

        private Result(T data, bool isSuccess, string message)
            : base(isSuccess, message)
        {
            Data = data;
        }
        public static Result<T> Success(T data, string message = "")
    => new Result<T>(data, true, message);

        public static new Result<T> Failure(string message)
            => new Result<T>(default!, false, message);
    }
}


   