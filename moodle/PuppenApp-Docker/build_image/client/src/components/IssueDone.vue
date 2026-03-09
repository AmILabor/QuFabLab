<template>
    <div>
        <b-form @reset="onReset" @submit="onSubmit">
            <b-form-group id="input-group-1" label="Lösung" label-for="input-1">
                <b-form-textarea
                        id="input-1"
                        v-model="form.resolution"
                        rows="3"
                        max-rows="6"
                        placeholder="Problemlösung angeben"
                        required
                ></b-form-textarea>
            </b-form-group>

            <b-form-group id="input-group-2" label="Kommentar" label-for="input-2">
                <b-form-textarea
                        id="input-2"
                        v-model="form.comment"
                        placeholder="(Optionalen) Kommentar hinzufügen"
                        rows="3"
                        max-rows="6"
                ></b-form-textarea>
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
        name: "IssueDone",
        props: [
            'puppetId',
            'issueId'
        ],
        data() {
            return {
                form: {
                    resolution: "",
                    comment: "",
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
            onSubmit(evt) {
                evt.preventDefault()

                let time = new Date().toISOString()
                time = time.substr(0, time.length-1) + "000Z"

                let body = {}
                for (let key in this.form) {
                    if (this.form[key] !== this.issue[key]) {
                        body[key] = this.form[key]
                    }
                }
                delete body.data
                body['last_edit'] = this.form['last_edit']

                let formData = new FormData()
                for (let key in body) {
                    formData.append(key, this.form[key])
                }
                formData.append("done", true)
                formData.append("resolution_date", time)

                fetch("/api/issues/" + this.issueId + "/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': this.$store.state.token,
                    },
                    // credentials: 'include',
                    method: "PATCH",
                    body: formData
                })
                    // .then(response => response.json())
                    .then(response => {
                        if (response.ok === true)
                            return response.json()
                        else
                            throw "response not ok"
                    })
                    .then(() => {
                        this.getPuppet(this.puppetId)
                        this.$bvModal.hide('modal-6-' + this.puppetId)
                        this.$emit('addsuccess')
                        }
                    )
                    .catch(error => this.$emit('adderror', error))
            },
            onCancel(evt) {
                evt.preventDefault()
                this.$bvModal.hide('modal-6-' + this.puppetId)
            },
            onReset(evt) {
                evt.preventDefault()
                this.form = {}
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
