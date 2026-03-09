<template>
    <b-card>
        <b-container fluid v-if="puppet != undefined">
            <b-row>
                <b-col cols="4">
                    <b-list-group>
                        <b-list-group-item>Nummer: {{ puppet.id }}</b-list-group-item>
                        <b-list-group-item>Name: {{ puppet.name }}</b-list-group-item>
                        <b-list-group-item>Serial: {{ puppet.serial }}</b-list-group-item>
                        <b-list-group-item>Anschluss: {{ puppet.connector }}</b-list-group-item>
                        <b-list-group-item>Ansprechpartner: {{ handler(puppet.handler).username }}</b-list-group-item>
                    </b-list-group>
                </b-col>
                <b-col cols="4">
                    <b-list-group>
                        <b-list-group-item>Haarfarbe: {{ hair_color(puppet.hair_color).name }}</b-list-group-item>
                        <b-list-group-item>Oberteilfarbe: {{ color(puppet.shirt_color).name }}</b-list-group-item>
                        <b-list-group-item>Oberteilname: {{ puppet.shirt_name }}</b-list-group-item>
                        <b-list-group-item>Hosenfarbe: {{ color(puppet.pants_color).name }}</b-list-group-item>
                        <b-list-group-item>Schuhfarbe: {{ color(puppet.shoe_color).name }}</b-list-group-item>
                    </b-list-group>
                </b-col>
                <b-col offset-lg="1" cols="2">
                    <b-img fluid :src="puppet.picture" v-if="puppet.picture" />
                </b-col>
                <b-col cols="1">
                    <b-button class="m-1" @click="$bvModal.show(editModalId)" v-b-tooltip.hover title="Puppe ändern">
                        <b-icon icon="pencil-square" />
                    </b-button>
                    <b-button class="m-1" @click="$bvModal.show(addIssueModalId)" v-b-tooltip:hover title="Problem hinzufügen">
                        <b-icon icon="file-plus" />
                    </b-button>
                </b-col>
            </b-row>
            <b-row class="pt-3" v-show="puppet.issues.length">
                <b-col>
                    <IssueList :puppet-id="puppetId" />
                </b-col>
            </b-row>
        </b-container>
        <b-modal hide-footer :id="editModalId" title="Puppe ändern">
            <PuppetEdit v-on:editerror="onPuppetEditError" v-on:editsuccess="onPuppetEditSuccess" :puppetId="puppetId" />
        </b-modal>
        <b-modal hide-footer :id="addIssueModalId" title="Problem hinzufügen">
            <IssueAdd v-on:adderror="onIssueAddError" v-on:addsuccess="onIssueAddSuccess" :puppetId="puppetId" />
        </b-modal>
    </b-card>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex'
    import IssueList from "@/components/IssueList";
    import PuppetEdit from "@/components/PuppetEdit";
    import IssueAdd from "@/components/IssueAdd";
    export default {
        name: "PuppetDetail",
        components: {PuppetEdit, IssueList, IssueAdd},
        props: [
            'puppetId'
        ],
        methods: {
            ...mapActions(['getPuppet']),
            uniqueModalId(id) {
                return "modal-" + id + "-" + this.puppetId
            },
            onPuppetEditError(error) {
                this.$bvToast.toast(error.stack, {
                    title: "Fehler",
                    noAutoHide: true,
                    variant: 'danger'
                })
            },
            onPuppetEditSuccess(name) {
                this.$bvToast.toast('Puppe ' + name + ' erfolgreich editiert', {
                    title: name,
                    autoHideDelay: 5000,
                    variant: 'success'
                })
            },
            onIssueAddError(error) {
                this.$bvToast.toast(error.stack, {
                    title: "Fehler",
                    noAutoHide: true,
                    variant: 'danger'
                })
            },
            onIssueAddSuccess() {
                this.$bvToast.toast('Issue erfolgreich hinzugefügt', {
                    title: 'Issue',
                    autoHideDelay: 5000,
                    variant: 'success'
                })
            }
        },
        created() {
            this.getPuppet(this.puppetId)
        },
        computed: {
            ...mapGetters([
                'detail',
                'color',
                'hair_color',
                'handler',
            ]),
            puppet() {
                return this.detail(this.puppetId)
            },
            editModalId() {
                return this.uniqueModalId('1')
            },
            addIssueModalId() {
                return this.uniqueModalId('4')
            },
        }
    }
</script>

<style scoped>

</style>