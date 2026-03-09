<template>
    <div>
        <b-form>
            <b-form-group id="input-group-1" label="Beschreibung" label-for="input-1">
                <b-form-textarea
                        id="input-1"
                        v-model="form.description"
                        placeholder="Problembeschreibung nicht angegeben"
                        rows="3"
                        max-rows="6"
                        readonly
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-2" label="Plan" label-for="input-2">
                <b-form-textarea
                        id="input-2"
                        v-model="form.plan"
                        placeholder="Lösungsvorgehen nicht angeben"
                        rows="3"
                        max-rows="6"
                        readonly
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-3" label="Ansprechpartner" label-for="input-3">
                <b-form-input
                        id="input-3"
                        v-model="mapHandlerOptions(handlers).find(handler => handler.value == form.handler).text"
                        placeholder="Ansprechpartner nicht hinterlegt"
                        readonly
                >
                </b-form-input>
            </b-form-group>

            <b-form-group id="input-group-4" label="Lösung" label-for="input-4">
                <b-form-textarea
                        id="input-4"
                        v-model="form.resolution"
                        placeholder="Lösungsvorgehen nicht angeben"
                        rows="3"
                        max-rows="6"
                        readonly
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-5" label="Kommentar" label-for="input-5">
                <b-form-textarea
                        id="input-5"
                        v-model="form.comment"
                        placeholder="Kein Kommentar hinterlegt"
                        rows="3"
                        max-rows="6"
                        readonly
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-6" label-for="input-6">
                <template v-slot:label>
                    Dateien
                </template>

                <b-input-group v-for="item in form.data" :key="item.id">
                    <b-form-input disabled :value="item.ref.split('/').pop()" />

                    <b-input-group-append>
                        <b-button target="_blank" :href="item.ref">
                            <b-icon :icon="getIconFor(item.ref)"/>
                        </b-button>
                    </b-input-group-append>
                </b-input-group>
            </b-form-group>
        </b-form>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex'
    export default {
        name: "IssueDetail",
        props: [
            'puppetId',
            'issueId'
        ],
        data() {
            return {
                form: {
                    description: null,
                    plan: null,
                    handler: null,
                    resolution: null,
                    comment: null,
                },
            }
        },
        computed: {
            ...mapGetters({
                handlers: 'handlers',
                getIssue: 'issue'
            }),
            issue() {
                return this.getIssue(this.puppetId, this.issueId)
            }
        },
        watch: {
            "issue": function(val) {
                this.form = Object.assign({}, val)
            }
        },
        methods: {
            ...mapActions(['getPuppet', 'getPuppets']),
            generateIndex(index) {
                return "input-" + (6 + index)
            },
            mapHandlerOptions(model) {
                return [{
                    value: null,
                    text: "kein Ansprechpartner"
                }].concat(model.map(entry => {
                    return {
                        text: entry.username,
                        value: entry.id
                    }
                }))
            },
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
        },
        created() {
            this.form = Object.assign({}, this.issue)
            this.getPuppet(this.puppetId)
        }
    }
</script>

<style scoped>

</style>
