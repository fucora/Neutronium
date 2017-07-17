﻿using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Dictionary;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.GlueObject
{
    public class JsGenericObject : GlueBase, IJSObservableBridge
    {   
        private IJavascriptObject _MappedJSValue;
        private readonly HybridDictionary<string, IJSCSGlue> _Attributes;

        public IReadOnlyDictionary<string, IJSCSGlue> Attributes => _Attributes;
        public IJavascriptObject MappedJSValue => _MappedJSValue;
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Object;      

        public JsGenericObject(object cValue, int childrenCount)
        {
            CValue = cValue;           
            _Attributes = new HybridDictionary<string, IJSCSGlue>(childrenCount);
        }

        public void GetBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestObjectCreation(_Attributes);
        }

        protected override void ComputeString(DescriptionBuilder context)
        {
            context.Append("{");
            var first = true;
            foreach (var it in _Attributes.OrderBy(kvp =>kvp.Key))
            {
                if (!first)
                    context.Append(",");

                context.Append($@"""{it.Key}"":");

                using (context.PushContext(it.Key))
                {                   
                    it.Value.BuilString(context);
                }

                first = false;           
            }

            context.Append("}");
        }

        public void SetMappedJSValue(IJavascriptObject jsobject)
        {
            _MappedJSValue = jsobject;
        }

        public override IEnumerable<IJSCSGlue> GetChildren()
        {
            return _Attributes.Values; 
        }

        public void AddGlueProperty(string propertyName, IJSCSGlue glue)
        {
            _Attributes.Add(propertyName, glue);
        }

        public void UpdateGlueProperty(string propertyName, IJSCSGlue glue)
        {
            _Attributes[propertyName] = glue;
        }

        public BridgeUpdater GetUpdater(string propertyName, IJSCSGlue glue)
        {
            UpdateGlueProperty(propertyName, glue);
            var context = new UpdateContext { ChildAllowWrite = !glue.IsBasic() };
            return new BridgeUpdater(viewModelUpdater => viewModelUpdater?.UpdateProperty(_MappedJSValue, propertyName, glue.GetJSSessionValue(), context));
        }
    }
}
