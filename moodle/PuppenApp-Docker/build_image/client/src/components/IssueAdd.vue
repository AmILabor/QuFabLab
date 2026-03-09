<template>
    <div>
        <b-form @reset="onReset" @submit="onSubmit">
            <b-form-group id="input-group-1" label="Beschreibung" label-for="input-1">
                <b-form-input
                        id="input-1"
                        v-model="form.description"
                        placeholder="Problembeschreibung angeben"
                        required
                        list="input-list"
                        autocomplete="off"
                        debounce="500"
                        v-on:update="onDebounce"
                ></b-form-input>
                <b-form-datalist id="input-list" :options="options" />
            </b-form-group>

            <b-form-group id="input-group-2" label="Plan" label-for="input-2">
                <b-form-textarea
                        id="input-2"
                        v-model="form.plan"
                        placeholder="Lösungsvorgehen angeben"
                        rows="3"
                        max-rows="6"
                        required
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-3" label="Ansprechpartner" label-for="input-3">
                <b-form-select
                        id="input-3"
                        v-model="form.handler"
                        placeholder="Ansprechpartner hinterlegen"
                        :options="mapHandlerOptions(handlers)"
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-4" label="Kommentar" label-for="input-4">
                <b-form-textarea
                        id="input-5"
                        v-model="form.comment"
                        placeholder="(Optionalen) Kommentar hinzufügen"
                        rows="3"
                        max-rows="6"
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-5" label-for="input-5">
                <template v-slot:label>
                    Dateien
                    <b-button class="ml-3" v-b-tooltip:hover title="Dateien hinzufügen" @click="addFile">
                        <b-icon icon="file-earmark-plus" />
                    </b-button>
                </template>

                <b-input-group v-for="(item, index) in files" :key="index"
                >
                    <b-form-file
                            :id="generateIndex(index)"
                            v-model="item.file"
                            placeholder="Datei hinzufügen"
                    ></b-form-file>
                    <b-input-group-append>
                        <b-button @click="removeFile(index)">
                            <b-icon icon="x"/>
                        </b-button>
                    </b-input-group-append>
                </b-input-group>
            </b-form-group>

            <footer id="modal-2___BV_modal_footer_" class="modal-footer">
                <b-button @click="onCancel" variant="secondary">Abbrechen</b-button>
                <b-button type="reset" variant="danger">Zurücksetzen</b-button>
                <b-button type="submit" variant="primary">Speichern</b-button>
            </footer>
        </b-form>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex'
    export default {
        name: "IssueAdd",
        props: [
            'puppetId'
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
                files: [{
                        file: null
                    }
                ],
                options: []
            }
        },
        computed: {
            ...mapGetters(['handlers'])
        },
        methods: {
            ...mapActions(['getPuppet', 'getPuppets']),
            onDebounce(value) {
                if (value.length < 3)
                    return;

                fetch("/api/autocompletion?text=" + value.trim(), {
                    headers: {
                        "Accept": "application/json",
                        "Authorization": this.$store.state.token
                    },
                    method: "GET"
                })
                    .then(response => response.json())
                    .then(data => this.options = data)
            },
            addFile() {
                this.files.push({file: null})
            },
            removeFile(index) {
                this.$set(this.files, index, {
                    file: null
                })
            },
            generateIndex(index) {
                return "input-" + (6 + index)
            },
            onSubmit(evt) {
                evt.preventDefault()

                let formData = new FormData()
                for (let key in this.form) {
                    formData.append(key, this.form[key])
                }
                formData.append('puppet', this.puppetId)
                fetch("/api/issues/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': this.$store.state.token,
                    },
                    // credentials: 'include',
                    method: "POST",
                    body: formData
                })
                    // .then(response => response.json())
                    .then(response => {
                        if (response.ok === true)
                            return response.json()
                        else
                            throw "response not ok"
                    })
                    .then(data => {
                            this.submitFiles(data.id)
                            this.getPuppet(this.puppetId)
                            this.getPuppets()
                            this.$bvModal.hide('modal-4-' + this.puppetId)
                            this.$emit('addsuccess')
                        }
                    )
                    .catch(error => this.$emit('adderror', error))
            },
            submitFiles(issueId) {
                for (let data of this.files) {
                    if (data.file != null) {
                        let formData = new FormData()
                        formData.append("title", "test")
                        formData.append("ref", data.file)
                        formData.append("issue", issueId)
                        fetch("/api/issuedata/", {
                            headers: {
                                'Accept': 'application/json',
                                'Authorization': this.$store.state.token,
                            },
                            // credentials: 'include',
                            method: "POST",
                            body: formData
                        })
                            .then(response => response.json())
                            .then(() => {
                                this.getPuppet(this.puppetId)
                            })
                    }
                }
            },
            onCancel(evt) {
                evt.preventDefault()
                this.$bvModal.hide('modal-4-' + this.puppetId)
            },
            onReset(evt) {
                evt.preventDefault()
                this.form = {}
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
            }
        }
    }
</script>

<style scoped>

</style>