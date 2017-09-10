using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using OneTrueError.MicroService.Core.Cqs;
using OneTrueError.MicroService.Core.Cqs.Control.Commands;
using OneTrueError.MicroService.Core.Cqs.Messages;
using Message = System.Messaging.Message;

namespace TestUI
{
    public partial class Form1 : Form
    {
        private readonly MessageQueue _inboundQueue;

        public Form1()
        {
            var queueName = @".\private$\OneTrueError.Service.Client";
            if (!MessageQueue.Exists(queueName))
                MessageQueue.Create(queueName);
            InitializeComponent();
            LoadContracts();


            _inboundQueue = new MessageQueue(queueName);
            _inboundQueue.MessageReadPropertyFilter.Extension = true;
            _inboundQueue.Formatter = new JsonMessageFormatter();
            _inboundQueue.ReceiveCompleted += OnReadCompleted;
            _inboundQueue.BeginReceive();

            var queue = new MessageQueue(@".\private$\OneTrueError.MessageBroker");
            var innerMsg = new Subscribe {AssemblyNames = new string[0]};
            var message = new Message(innerMsg, new JsonMessageFormatter());
            message.ResponseQueue = _inboundQueue;
            var ext = new MessageExtension("Client", innerMsg, 1) {FrameMethod = "SUBSCRIBE"};
            message.Extension = ext.Serialize();
            queue.Send(message);

        }

        private void OnReadCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var msg = _inboundQueue.EndReceive(e.AsyncResult);
            Invoke(new Action(() =>
            {
                tbResult.Text += DateTime.Now.ToLongDateString() + " " +
                                 JsonConvert.SerializeObject(msg.Body, Formatting.Indented) + "\r\n\r\n";
            }));
            _inboundQueue.BeginReceive();
        }

        private void comboKeyPressed()
        {
            comboBox1.DroppedDown = true;

            var originalList = (object[]) comboBox1.Tag;
            if (originalList == null)
            {
                // backup original list
                originalList = new object[comboBox1.Items.Count];
                comboBox1.Items.CopyTo(originalList, 0);
                comboBox1.Tag = originalList;
            }

            // prepare list of matching items
            var s = comboBox1.Text.ToLower();
            IEnumerable<object> newList = originalList;
            if (s.Length > 0)
            {
                newList = originalList.Where(item => item.ToString().ToLower().Contains(s));
            }

            // clear list (loop through it, otherwise the cursor would move to the beginning of the textbox...)
            comboBox1.BeginUpdate();
            while (comboBox1.Items.Count > 0)
            {
                comboBox1.Items.RemoveAt(0);
            }

            // re-set list
            comboBox1.Items.AddRange(newList.ToArray());
            comboBox1.EndUpdate();
        }

        public class TypeWrapper
        {
            private readonly Type _type;
            public TypeWrapper(Type type)
            {
                _type = type;
            }

            public Type DtoType
            {
                get { return _type; }
            }

            public override string ToString()
            {
                return _type.FullName.Replace("OneTrueError.", "").Replace("Api.", "");
            }
        }
        public void LoadContracts()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pos = path.IndexOf("\\v3\\");
            path = path.Substring(0, pos) + "\\v3\\Contracts";
            var files = Directory.GetFiles(path, "*.dll");
            foreach (var file in files)
            {
                if (file.IndexOf("api", StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                var asm = Assembly.LoadFile(file);
                try
                {
                    foreach (var type in asm.GetTypes())
                    {
                        comboBox1.Items.Add(new TypeWrapper(type));
                        if (type.Name == "GetSimilarities")
                            comboBox1.SelectedItem = type;
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var exs = string.Join(",", ex.LoaderExceptions.Select(x => x.Message));
                    MessageBox.Show(exs);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = comboBox1.SelectedItem as TypeWrapper;
            if (type == null)
                return;

            var dto = BuildDto(type.DtoType);

            if (Environment.MachineName.Equals("JONASBARBAR"))
                fastColoredTextBox1.Text = JsonConvert.SerializeObject(dto, Formatting.Indented).Replace("60", "2");
            else
                fastColoredTextBox1.Text = JsonConvert.SerializeObject(dto, Formatting.Indented).Replace("60", "21");
        }

        private static object BuildDto(Type type)
        {
            var dto = CreateDto(type);
            if (dto == null)
                return null;

            foreach (var propertyInfo in dto.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.Namespace.StartsWith("OneTrueError"))
                {
                    if (propertyInfo.PropertyType.IsArray)
                    {
                        var arr = Array.CreateInstance(propertyInfo.PropertyType.GetElementType(), 1);
                        var arrItem = BuildDto(propertyInfo.PropertyType.GetElementType());
                        arr.SetValue(arrItem, 0);
                        propertyInfo.SetValue(dto, arr);
                        continue;
                    }

                    var subObject = BuildDto(propertyInfo.PropertyType);
                    propertyInfo.SetValue(dto, subObject);
                }
                else if (propertyInfo.PropertyType == typeof (string))
                {
                    propertyInfo.SetValue(dto, "Hello");
                }
            }
            return dto;
        }

        private static object CreateDto(Type type)
        {
            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                null, new Type[0], null);
            if (constructor == null)
            {
                MessageBox.Show("Failed to find a default constructor for " + type.FullName);
                return null;
            }

            var dto = constructor.Invoke(new object[0]);
            return dto;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var wrapper = comboBox1.SelectedItem as TypeWrapper;
            if (wrapper == null)
            {
                MessageBox.Show("Välj en typ");
                return;
            }

            var dto = JsonConvert.DeserializeObject(fastColoredTextBox1.Text, wrapper.DtoType);
            var msg = new Message(dto, new JsonMessageFormatter());
            var ext = Environment.MachineName.Equals("JONASBARBAR")
                ? new MessageExtension("Client", dto, 1)
                : new MessageExtension("Client", dto, 3);
            msg.ResponseQueue = _inboundQueue;
            msg.Extension = ext.Serialize();

            var queue = new MessageQueue(@".\private$\OneTrueError.MessageBroker");
            queue.Formatter = new JsonMessageFormatter();
            queue.Send(msg);
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                return;
            }
            comboKeyPressed();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length == 0) comboKeyPressed();
        }
    }
}