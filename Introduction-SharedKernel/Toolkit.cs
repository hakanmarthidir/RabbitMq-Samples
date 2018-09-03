using System;

namespace Introduction_SharedKernel
{
    public static class Toolkit
    {
        public static byte[] GetBytes<T>(T model)
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            return System.Text.Encoding.UTF8.GetBytes(serialized);
        }

        public static T GetModel<T>(byte[] modelByte)
        {
            var stringModel = System.Text.Encoding.UTF8.GetString(modelByte);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(stringModel);
        }

    }
}
