<template>
    <div>
        <div v-if="token">
            <Header/>
            <b-table striped hover :fields="fields" :items="items" @row-clicked="onRowClicked">
                <template v-slot:cell(issue_count_open)="row">
                    {{ row.item['issue_count_open'] }} / {{ row.item['issue_count'] }}
                </template>
                <template v-slot:cell(show_details)="row">
                    <b-button size="sm" @click="row.toggleDetails" class="mr-2">
                        <b-icon :icon="row.detailsShowing ? 'arrows-angle-contract' : 'arrows-angle-expand'" />
                    </b-button>
                </template>

                <template v-slot:row-details="row">
                    <PuppetDetail :puppetId="row.item.id" />
                </template>
            </b-table>

            <div>
                <b-button size="sm" @click="logout()">
                    Logout
                </b-button>
            </div>

            <b-modal hide-footer id="modal-3" title="Puppe hinzufügen">
                <PuppetAdd v-on:adderror="onPuppetAddError" v-on:addsuccess="onPuppetAddSuccess" />
            </b-modal>
        </div>
        <div>
            <b-modal no-close-on-backdrop no-close-on-esc hide-footer id="modal-0" title="Login">
                <template v-slot:modal-header>
                    <b-card-title>Login Puppenakte</b-card-title>
                </template>
                <Login v-on:loginerror="onLoginError" />
            </b-modal>
        </div>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from 'vuex'
    import PuppetDetail from "@/components/PuppetDetail";
    import PuppetAdd from "@/components/PuppetAdd";
    import Login from "@/components/Login";
    import Header from "@/components/Header";

    export default {
        name: "PuppetList",
        components: {
            Header,
            PuppetDetail,
            PuppetAdd,
            Login
        },
        computed: {
            ...mapGetters([
                'puppets',
                'token'
            ]),
        },
        methods: {
            ...mapActions([
                'getPuppets',
                'getColors',
                'getHairColors',
                'getHandlers'
            ]),
            onPuppetAddError(error) {
                this.$bvToast.toast(error.stack, {
                    title: "Fehler",
                    noAutoHide: true,
                    variant: 'danger'
                })
            },
            onPuppetAddSuccess(name) {
                this.$bvToast.toast('Puppe ' + name + ' erfolgreich hinzugefügt', {
                    title: name,
                    autoHideDelay: 5000,
                    variant: 'success'
                })
            },
            onLoginError(error) {
                console.log(error)
                this.$bvToast.toast(error["non_field_errors"], {
                    title: "Login",
                    autoHideDelay: 10000,
                    variant: 'danger'
                })
            },
            logout() {
                fetch("/api/session", {
                    headers: {
                        "Accept": "application/json",
                        "Authorization": this.$store.state.token,
                    },
                    method: "DELETE",
                })
                .then(response => {
                    this.$store.commit('setToken', null)
                    if (response.status !== 204) {
                        console.log("logout error code: " + response.status)
                    }
                })
            },
            onRowClicked(row) {
                this.$set(row, '_showDetails', !row._showDetails)
            }
        },
        watch: {
            token: function(val) {
                if (!val) {
                    this.$bvModal.show('modal-0')
                }
                else {
                    this.getPuppets()
                        .then(() => this.getColors())
                        .then(() => this.getHairColors())
                        .then(() => this.getHandlers())
                }
            },
            puppets: function(val) {
                if (!this.items.length) {
                    this.items = JSON.parse(JSON.stringify(val))
                }
                else {
                    let detailedPuppets = this.items.filter(
                        puppet => puppet._showDetails === true
                    ).map(
                        puppet => puppet.id
                    )
                    this.items = JSON.parse(JSON.stringify(val))
                    let matchingIndices = this.items.filter(
                        puppet => detailedPuppets.includes(puppet.id)
                    ).map(
                        puppet => this.items.indexOf(puppet)
                    )
                    for (const idx of matchingIndices) {
                        this.$set(this.items, idx, {
                            ...this.items[idx],
                            _showDetails: true
                        })
                    }
                }
            }
        },
        data() {
            return {
                items: [],
                fields: [
                    {
                        key: 'id',
                        sortable: true,
                        label: "Nummer"
                    },
                    {
                        key: 'name',
                        sortable: true,
                        label: "Name"
                    },
                    {
                        key: 'handler.username',
                        sortable: true,
                        label: "Ansprechpartner"
                    },
                    {
                        key: 'issue_count_open',
                        sortable: true,
                        label: "Probleme"
                    },
                    {
                        key: 'handler.location',
                        sortable: true,
                        label: "Ort"
                    },
                    {
                        key: 'show_details',
                        sortable: false,
                        label: "Details"
                    },
                ]
            }
        },
        mounted() {
            let token = window.localStorage.getItem("authToken")
            if (token)
                this.$store.commit('setToken', token)
            else
                this.$bvModal.show('modal-0')
        }
    }
</script>

<style scoped>

</style>