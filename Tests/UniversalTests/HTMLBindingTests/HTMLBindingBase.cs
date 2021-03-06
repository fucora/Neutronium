﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Neutronium.Core;
using Neutronium.Core.Binding;
using Neutronium.Core.Test.Helper;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Example.ViewModel;
using NSubstitute;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Xunit.Abstractions;

namespace Tests.Universal.HTMLBindingTests
{
    public class HtmlBindingBase : IntegratedTestBase 
    {
        protected readonly Person _DataContext;
        protected ICommand _Command;

        public static IEnumerable<object[]> BasicVmData
        {
            get
            {
                yield return new object[] { new BasicTestViewModel() };
                yield return new object[] { null };
            }
        }

        protected Task<IHtmlBinding> Bind(HtmlViewEngine engine, object dataContext, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay)
        {
            return HtmlBinding.Bind(engine, dataContext, mode);
        }

        protected HtmlBindingBase(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
            _Command = Substitute.For<ICommand>();
            _DataContext = new Person(_Command) {
                Name = "O Monstro",
                LastName = "Desmaisons",
                Local = new Local() { City = "Florianopolis", Region = "SC" },
                PersonalState = PersonalState.Married
            };

            _DataContext.Skills.Add(new Skill() { Name = "Langage", Type = "French" });
            _DataContext.Skills.Add(new Skill() { Name = "Info", Type = "C++" });
        }

        protected PerformanceHelper GetPerformanceCounter(string description) => new PerformanceHelper(_TestOutputHelper, description);

        protected void CheckCollection(IJavascriptObject coll, IList<Skill> skill) 
        {
            coll.GetArrayLength().Should().Be(skill.Count);

            for (var  i = 0; i < skill.Count; i++) 
            {
                var c = coll.GetValue(i);

                (GetSafe(() => GetStringAttribute(c, "Name"))).Should().Be(skill[i].Name);
                (GetSafe(() => GetStringAttribute(c, "Type"))).Should().Be(skill[i].Type);
            }
        }
    }
}
