<template>
    <div>
        <b-table striped hover :fields="fields" :items="issues" sort-by="done">
            <template v-slot:cell(done)="row">
                <b-form-checkbox :checked="row.item.done" disabled />
            </template>

            <template v-slot:cell(description)="row">
                {{ row.item.description.length > 128
                    ? row.item.description.substring(0, 124) + " ..."
                    : row.item.description
                }}
            </template>

            <template v-slot:cell(data)="row">
                <b-button v-for="item of getIssueDataIcons(row.item.data)" :key="item.url" target="_blank" :href="item.url" class="m-1">
                    <b-icon :icon="item.icon" />
                </b-button>
            </template>

            <template v-slot:cell(actions)="row">
                <b-button class="m-1" @click="detailIssue(row.item.id)" v-b-tooltip:hover title="Problem anzeigen">
                    <b-icon icon="info-square" />
                </b-button>

                <b-button class="m-1" @click="editIssue(row.item.id)" v-b-tooltip:hover title="Problem ändern">
                    <b-icon icon="pencil-square" />
                </b-button>

                <b-button class="m-1" @click="doneIssue(row.item.id)" v-b-tooltip:hover title="Problem lösen">
                    <b-icon icon="check" />
                </b-button>

                <b-button class="m-1" @click="showDeleteModal(row.item.id)" v-b-tooltip:hover title="Problem löschen">
                    <b-icon icon="trash" />
                </b-button>
            </template>
        </b-table>
        <b-modal ok-title="Schließen" ok-only :id="detailIssueModalId" title="Problem anzeigen">
            <IssueDetail :puppetId="puppetId" :issueId="issueId"/>
        </b-modal>

        <b-modal hide-footer :id="editIssueModalId" title="Problem ändern">
            <IssueEdit v-on:adderror="onIssueEditError" v-on:addsuccess="onIssueEditSuccess" :puppetId="puppetId" :issueId="issueId"/>
        </b-modal>

        <b-modal hide-footer :id="doneIssueModalId" title="Problem lösen">
            <IssueDone v-on:adderror="onIssueEditError" v-on:addsuccess="onIssueEditSuccess" :puppetId="puppetId" :issueId="issueId"/>
        </b-modal>

        <b-modal cancel-title="Nein" ok-title="Ja" ok-variant="danger" @ok="deleteIssue()" :id="deleteIssueModalId" title="Problem löschen">
            Soll das Problem wirklich unwiderruflich gelöscht werden?
        </b-modal>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex'
    import IssueEdit from "@/components/IssueEdit";
    import IssueDone from "@/components/IssueDone";
    import IssueDetail from "@/components/IssueDetail";

    export default {
        name: "IssueList",
        components: {IssueEdit, IssueDone, IssueDetail},
        props: [
            'puppetId'
        ],
        computed: {
            ...mapGetters({
                issuesFor: 'issues',
                handlers: 'handlers'
            }),
            issues() {
                return this.issuesFor(this.puppetId)
            },
            editIssueModalId() {
                return this.uniqueModalId('5')
            },
            doneIssueModalId() {
                return this.uniqueModalId('6')
            },
            detailIssueModalId() {
                return this.uniqueModalId('7')
            },
            deleteIssueModalId() {
                return this.uniqueModalId('8')
            },
        },
        methods: {
            ...mapActions(['getPuppet']),
            getIconFor(file) {
                const videos = ['mkv', 'mov', 'mp4', 'webm', 'gif', 'wmv', 'flv']
                const images = ['webp', 'jpg', 'jpeg', 'png']
                const documents = ['docx', 'pdf', 'tiff']
                const sheets= ['xlsx']
                const ext = file.split('.').pop()

                if (videos.includes(ext))
                    return 'camera-video'
                if (images.includes(ext))
                    return 'image'
                if (documents.includes(ext))
                    return 'file-earmark-text'
                if (sheets.includes(ext))
                    return 'file-earmark-spreadsheet'
                return 'file-earmark'
            },
            getIssueDataIcons(data) {
                return data.map(entry => {
                    return {
                        icon: this.getIconFor(entry.ref),
                        url: entry.ref
                    }
                })
            },
            uniqueModalId(id) {
                return "modal-" + id + "-" + this.puppetId
            },
            onIssueEditError(error) {
                this.$bvToast.toast(error.stack, {
                    title: "Fehler",
                    noAutoHide: true,
                    variant: 'danger'
                })
            },
            onIssueEditSuccess() {
                this.$bvToast.toast('Issue erfolgreich bearbeitet', {
                    title: 'Issue',
                    autoHideDelay: 5000,
                    variant: 'success'
                })
            },
            editIssue(id) {
                this.issueId = id
                this.$bvModal.show(this.editIssueModalId)
            },
            doneIssue(id) {
                this.issueId = id
                this.$bvModal.show(this.doneIssueModalId)
            },
            detailIssue(id) {
                this.issueId = id
                this.$bvModal.show(this.detailIssueModalId)
            },
            showDeleteModal(id) {
                this.issueId = id
                this.$bvModal.show(this.deleteIssueModalId)
            },
            deleteIssue() {
                fetch("/api/issues/" + this.issueId + "/", {
                    headers: {
                        "Accept": "application/json",
                        "Authorization": this.$store.state.token,
                    },
                    method: "DELETE",
                })
                    .then(() => {
                        this.issueId = null
                        this.getPuppet(this.puppetId)
                    })
            },
        },
        data() {
            return {
                fields: [
                    {
                        key: 'done',
                        sortable: true,
                        label: "Erledigt"
                    },
                    {
                        key: 'id',
                        sortable: true,
                        label: "Nummer"
                    },
                    {
                        key: 'creator',
                        sortable: true,
                        label: "Ersteller",
                        formatter: value => this.handlers.find(handler => handler.id === value).username,
                        sortByFormatted: true
                    },
                    {
                        key: 'description',
                        sortable: false,
                        label: "Beschreibung"
                    },
                    {
                        key: 'data',
                        sortable: false,
                        label: "Dateien"
                    },
                    {
                        key: 'actions',
                        sortable: false,
                        label: "Aktionen"
                    }
                ],
                issueId: null,
            }
        },
    }
</script>

<style scoped>

</style>