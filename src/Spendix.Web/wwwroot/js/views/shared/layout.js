﻿var vm;

$(document).ready(function () {
    vm = new Vue({
        el: '#accordionSidebar',
        data: {
            loading: true,
            bankAccounts: []
        },
        methods: {
            getBankAccounts() {
                fetch('/api/BankAccounts')
                    .then(
                        function (response) {
                            if (response.status !== 200) {
                                console.log('Error. Status Code: ' + response.status);
                                return;
                            }

                            response.json().then(function (data) {
                                vm.loading = false;
                                vm.bankAccounts = data;
                            });
                        }
                    )
                    .catch(function (err) {
                        console.log('Fetch Error :-S', err);
                    });
            },
            toggleSidebar() {
                $("body").toggleClass("sidebar-toggled");
                $(".sidebar").toggleClass("toggled");
                if ($(".sidebar").hasClass("toggled")) {
                    $('.sidebar .collapse').collapse('hide');
                };
            }
        }
    });

    vm.getBankAccounts();
});