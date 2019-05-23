var sidebarViewModel;
var editBankAccountModalViewModel;

$(document).ready(function () {
    sidebarViewModel = new Vue({
        el: '#accordionSidebar',
        data: {
            loading: true,
            bankAccounts: [],
            selectedBankAccountId: '',
            showActions: true
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
                                sidebarViewModel.loading = false;
                                sidebarViewModel.bankAccounts = data;
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
                }

                this.showActions = !$(".sidebar").hasClass("toggled");
            },
            showAddAccountModal() {
                editBankAccountModalViewModel.clearBankAccount();
                $('#editBankAccountModal').modal('show');
            },
            showEditBankAccountModal(bankAccountId) {
                var bankAccount = this.bankAccounts.filter(function (bankAccount) {
                    return bankAccount.bankAccountId === bankAccountId;
                })[0];

                editBankAccountModalViewModel.setBankAccount(bankAccount);
                $('#editBankAccountModal').modal('show');
            },
            showDeleteBankAccountModal(bankAccountId) {
                this.selectedBankAccountId = bankAccountId;
                $('#deleteBankAccountModal').modal('show');
            },
            deleteBankAccount() {
                fetch('/api/BankAccounts', {
                    method: 'DELETE',
                    body: JSON.stringify(this.selectedBankAccountId),
                    headers: {
                        'Content-Type': 'application/json'
                    },
                }).then(function (response) {
                    if (response.status !== 200) {
                        console.log('Error. Status Code: ' + response.status);
                        return;
                    }

                    sidebarViewModel.bankAccounts = sidebarViewModel.bankAccounts.filter(function (bankAccount) {
                        return bankAccount.bankAccountId !== sidebarViewModel.selectedBankAccountId;
                    });

                    $('#deleteBankAccountModal').modal('hide');
                }).catch(err => console.log('Fetch Error :-S', err));
            }
        }
    });

    editBankAccountModalViewModel = new Vue({
        el: '#editBankAccountModal',
        data: {
            bankAccountId: '',
            name: '',
            type: 'Checking',
            openingBalance: 0
        },
        methods: {
            setBankAccount(bankAccount) {
                this.bankAccountId = bankAccount.bankAccountId;
                this.name = bankAccount.name;
                this.type = bankAccount.type;
                this.openingBalance = bankAccount.openingBalance;
            },
            clearBankAccount() {
                this.bankAccountId = '';
                this.name = '';
                this.type = 'Checking';
                this.openingBalance = 0;
            },
            saveBankAccount() {
                if ($('#editBankAccountForm').valid() === false) {
                    return;
                }

                var data = {
                    bankAccountId: this.bankAccountId,
                    name: this.name,
                    type: this.type,
                    openingBalance: this.openingBalance
                };

                fetch('/api/BankAccounts', {
                    method: 'POST',
                    body: JSON.stringify(data),
                    headers: {
                        'Content-Type': 'application/json'
                    },
                }).then(function (response) {
                    if (response.status !== 200) {
                        console.log('Error. Status Code: ' + response.status);
                        return;
                    }

                    sidebarViewModel.getBankAccounts();

                    $('#editBankAccountModal').modal('hide');
                }).catch(err => console.log('Fetch Error :-S', err));
            }
        }
    });

    sidebarViewModel.getBankAccounts();

    $('#editBankAccountForm').validate({
        rules: {
            type: {
                required: true
            },
            name: {
                required: true
            },
            openingBalance: {
                required: true,
                number: true
            }
        },
        messages: {
            type: {
                required: 'Account Type is required.'
            },
            name: {
                required: 'Account Name is required.'
            },
            openingBalance: {
                required: 'Opening Balance is required.',
                number: 'Opening Balance must be a valid number.'
            }
        }
    });
});