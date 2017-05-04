using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Seagull2.StraPosit.WebApi.Utility
{
    public class ViewModelManager
    {
        public static readonly ViewModelManager Instance = new ViewModelManager();
        private ValidatorHandler vHandler = new ValidatorHandler();

        public ViewModelManager()
        {
        }

        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private string tmpResult = string.Empty;

        /// <summary>
        /// 把验证器的节点添加到指定对象上
        /// </summary>
        /// <typeparam name="T">获取验证器的类型</typeparam>
        /// <param name="data">被添加验证器的object</param>
        /// <returns></returns>
        public object AppendValidator<T>(object data)
        {
            //序列化属性小写配置
            tmpResult = string.Empty;
            tmpResult = JsonConvert.SerializeObject(data, settings);
            JObject viewModel = (JObject)JsonConvert.DeserializeObject(tmpResult);

            tmpResult = JsonConvert.SerializeObject(vHandler.CreateValidators<T>(), settings);
            viewModel.Add("validators", (JArray)JsonConvert.DeserializeObject(tmpResult));
            return viewModel;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">需要获取验证器的类型</typeparam>
        /// <param name="data">操作主体</param>
        /// <param name="key">节点名</param>
        /// <param name="appendData">节点数据数组</param>
        /// <returns></returns>
        public object AppendValidatorAndObject<T>(object data, string key, object appendData)
        {
            //序列化属性小写配置
            tmpResult = string.Empty;
            tmpResult = JsonConvert.SerializeObject(data, settings);
            JObject viewModel = (JObject)JsonConvert.DeserializeObject(tmpResult);

            tmpResult = JsonConvert.SerializeObject(vHandler.CreateValidators<T>(), settings);
            viewModel.Add("validators", (JArray)JsonConvert.DeserializeObject(tmpResult));

            tmpResult = JsonConvert.SerializeObject(appendData, settings);
            viewModel.Add(key, (JArray)JsonConvert.DeserializeObject(tmpResult));
            return viewModel;
        }
    }
}