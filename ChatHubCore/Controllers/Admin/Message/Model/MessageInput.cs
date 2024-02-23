using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHubApi.Controllers.AdminServices.Message.Model
{
    public class MessageInput
    {
        public virtual int id { get; set; }
        public virtual string Sender { get; set; }
        public virtual string Receiver { get; set; }

        public virtual string SendMessage { get; set; }
    }

    public class AddMessageInput : MessageInput
    {
        [Required(ErrorMessage = "发送者不能为空")]
        public override string Sender { get; set; }

        [Required(ErrorMessage = "接受者不能为空")]
        public override string Receiver { get; set; }

        [Required(ErrorMessage = "消息体不能为空")]
        public override string SendMessage { get; set; }
    }

    public class EditMessageInput : MessageInput
    {
        [Required(ErrorMessage = "发送者不能为空")]
        public override string Sender { get; set; }

        [Required(ErrorMessage = "接受者不能为空")]
        public override string Receiver { get; set; }

        [Required(ErrorMessage = "消息体不能为空")]
        public override string SendMessage { get; set; }
    }
}
