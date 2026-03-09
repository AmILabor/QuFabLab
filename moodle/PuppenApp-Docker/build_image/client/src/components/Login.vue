<template>
    <b-form @submit="onSubmit">
        <b-form-group id="input-group-1" label="Benutzername" label-for="input-1">
            <b-form-input
                    id="input-1"
                    v-model="form.username"
                    placeholder="Benutzernamen angeben"
                    required
            ></b-form-input>
        </b-form-group>
        <b-form-group id="input-group-2" label="Passwort" label-for="input-2">
            <b-form-input
                id="input-2"
                v-model="form.password"
                placeholder="Passwort angeben"
                type="password"
                required
            ></b-form-input>
        </b-form-group>

        <footer id="modal-2___BV_modal_footer_" class="modal-footer">
            <b-button type="submit" variant="primary">Start</b-button>
        </footer>
    </b-form>
</template>

<script>
    export default {
        name: "Login",
        data() {
            return {
                form: {
                    username: "",
                    password: ""
                }
            }
        },
        methods: {
            onSubmit(evt) {
                evt.preventDefault()

                fetch("/api-auth/", {
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    method: "POST",
                    body: JSON.stringify(this.form)
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.token) {
                            this.$store.commit('setToken', data.token)
                            this.$bvModal.hide('modal-0')
                        } else {
                            this.$emit('loginerror', data)
                        }
                    })
                    .catch(error => {
                        this.$emit('loginerror', error)
                    })
            },
            onCancel(evt) {
                evt.preventDefault()
            }
        },
    }
</script>

<style scoped>

</style>