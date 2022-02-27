﻿using System;
using System.Threading.Tasks;

namespace ContextMenuCustomApp.View.Common
{
    public abstract class BaseViewModel : BaseModel
    {
        private bool busy;
        public bool IsBusy { get => busy; set => SetProperty(ref busy, value); }

        private string message;
        public string Message { get => message; set => SetProperty(ref message, value); }

        public delegate void MessageEventHandler(string message, Exception exception);
        public event MessageEventHandler Handler;

        public void OnError(Exception e, string message = null)
        {
            Message = message ?? e.Message;
            Handler?.Invoke(message, e);
        }

        public void OnMessage(string message)
        {
            Handler?.Invoke(message, null);
        }

        public void Busy(bool busy, string message = null)
        {
            IsBusy = busy;
            Message = message ?? string.Empty;
        }

        public async Task RunWith(Func<Task> action)
        {
            Busy(true);
            Message = string.Empty;
            try
            {
                await action();
            }
            catch (Exception e)
            {
                Message = e.Message;
                Handler?.Invoke(Message, e);
            }
            finally
            {
                Busy(false);
            }
        }

        public async Task<T> RunWith<T>(Func<Task<T>> action)
        {
            Busy(true);
            Message = string.Empty;
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                Message = e.Message;
                Handler?.Invoke(Message, e);
            }
            finally
            {
                Busy(false);
            }

            return default;
        }
    }
}
