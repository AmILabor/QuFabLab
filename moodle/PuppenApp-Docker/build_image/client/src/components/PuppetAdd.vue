<template>
    <div>
        <b-form @reset="onReset" @submit="onSubmit">
            <b-form-group id="input-group-1" label="Name" label-for="input-1">
                <b-form-input
                        id="input-1"
                        v-model="form.name"
                        placeholder="Enter name"
                        required
                ></b-form-input>
            </b-form-group>

            <b-form-group id="input-group-2" label="Serial" label-for="input-2">
                <b-form-input
                        id="input-2"
                        v-model="form.serial"
                        placeholder="Enter serial"
                        required
                ></b-form-input>
            </b-form-group>

            <b-form-group id="input-group-3" label="Connector" label-for="input-3">
                <b-form-select
                        id="input-3"
                        v-model="form.connector"
                        :options="connectors"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-4" label="Ansprechpartner" label-for="input-4">
                <b-form-select
                        id="input-4"
                        v-model="form.handler"
                        :options="mapHandlerOptions(handlers)"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-5" label="Oberteilname" label-for="input-5">
                <b-form-input
                        id="input-5"
                        v-model="form.shirt_name"
                        placeholder="Enter shirtname"
                        required
                ></b-form-input>
            </b-form-group>

            <b-form-group id="input-group-6" label="Haarfarbe" label-for="input-6">
                <b-form-select
                        id="input-6"
                        v-model="form.hair_color"
                        :options="mapColorOptions(hair_colors)"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-7" label="Oberteilfarbe" label-for="input-7">
                <b-form-select
                        id="input-7"
                        v-model="form.shirt_color"
                        :options="mapColorOptions(colors)"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-8" label="Hosenfarbe" label-for="input-8">
                <b-form-select
                        id="input-8"
                        v-model="form.pants_color"
                        :options="mapColorOptions(colors)"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-9" label="Schuhfarbe" label-for="input-9">
                <b-form-select
                        id="input-9"
                        v-model="form.shoe_color"
                        :options="mapColorOptions(colors)"
                        required
                ></b-form-select>
            </b-form-group>

            <b-form-group id="input-group-10" label="Bild" label-for="input-10">
                <b-form-file
                        id="input-10"
                        v-model="form.picture"
                        placeholder="Enter picture"
                        accept="image/*"
                        required
                ></b-form-file>
            </b-form-group>

            <footer id="modal-2___BV_modal_footer_" class="modal-footer">
                <b-button @click="onCancel" variant="secondary">Cancel</b-button>
                <b-button type="reset" variant="danger">Reset</b-button>
                <b-button type="submit" variant="primary">Submit</b-button>
            </footer>
        </b-form>
    </div>
</template>

<script>
    import {mapActions, mapGetters} from "vuex";

    export default {
        name: "PuppetAdd",
        data() {
            return {
                form: {},
                connectors: [{ text: 'Select One', value: null }, 'USB', 'Strom'],
            }
        },
        computed: {
            ...mapGetters([
                'hair_colors',
                'colors',
                'handlers',
            ]),
        },
        methods: {
            ...mapActions(['getPuppet', 'getPuppets']),
            onSubmit(evt) {
                evt.preventDefault()

                let formData = new FormData()
                for (let key in this.form) {
                    formData.append(key, this.form[key])
                }
                fetch("/api/puppet/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': this.$store.state.token,
                    },
                    // credentials: 'include',
                    method: "POST",
                    body: formData
                })
                    .then(response => {
                        if (response.ok === true) {
                            this.getPuppets()
                            this.$bvModal.hide('modal-3')
                            this.$emit('addsuccess', this.form.name)
                        }
                    })
                    .catch(error => this.$emit('adderror', error))
            },
            onReset(evt) {
                evt.preventDefault()
                this.form = {}
            },
            onCancel(evt) {
                evt.preventDefault()
                this.$bvModal.hide('modal-3')
            },
            mapColorOptions(model) {
                return model.map(entry => {
                    return {
                        text: entry.name,
                        value: entry.id
                    }
                })
            },
            mapHandlerOptions(model) {
                return model.map(entry => {
                    return {
                        text: entry.username,
                        value: entry.id
                    }
                })
            },
        },
    }
</script>

<style scoped>

</style>