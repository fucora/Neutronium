﻿(function () {
    const localMixin = {
        data: {
            factoryresult: null,
            lastError: null,
            inputName: null
        },
        methods: {
            click: function () {
                var self = this;
                this.asPromise(this.ViewModel.CreateObject)(this.ViewModel.inputName)
                    .then(function (res) {
                        console.log(res);
                        alert(res.LastName);
                        window.res = res;
                        self.factoryresult = res;
                    })
                    .catch(function (reason) {
                        console.log(reason);
                        alert(reason);
                    });
            },
            error: function (error) {
                alert(error);
                this.lastError = error;
            },
            result: function (res) {
                alert(res.LastName);
                this.factoryresult = res;
            }
        }
    };

    window.glueHelper.setOption({
        mixins: [window.glueHelper.promiseMixin, localMixin]
    });

})()