import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export const store = new Vuex.Store({
    state: {
        puppets: [],
        details: {},
        token: null,
        colors: [],
        hair_colors: [],
        handlers: [],
    },
    mutations: {
        setPuppets(state, puppets) {
            state.puppets = puppets
        },
        setPuppet(state, puppet) {
            Vue.set(state.details, puppet.id, puppet)
        },
        setColors(state, colors) {
            state.colors = colors
        },
        setHairColors(state, hair_colors) {
            state.hair_colors = hair_colors
        },
        setHandlers(state, handlers) {
            state.handlers = handlers
        },
        setToken(state, token) {
            if (token) {
                state.token = "Token " + token
                window.localStorage.setItem("authToken", token)
            }
            else {
                state.token = null
                window.localStorage.removeItem("authToken")
            }
        }
    },
    actions: {
        getPuppets({commit, state}) {
            if (state.token) {
                return fetch("/api/puppets/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': state.token
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.detail == "Invalid token.")
                            throw data.detail
                        commit('setPuppets', data)
                    })
                    .catch(error => {
                        console.log(error)
                        commit('setToken', null)
                    })
            }
        },
        getPuppet({commit, state}, id) {
            if (state.token) {
                return fetch("/api/puppet/" + id + "/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': state.token
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.detail == "Invalid token.")
                            throw data.detail
                        commit('setPuppet', data)
                    })
                    .catch(error => {
                        console.log(error)
                        commit('setToken', null)
                    })
            }
        },
        getColors({commit, state}) {
            if (state.token) {
                return fetch("/api/colors/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': state.token
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.detail == "Invalid token.")
                            throw data.detail
                        commit('setColors', data)
                    })
                    .catch(error => {
                        console.log(error)
                        commit('setToken', null)
                    })
            }
        },
        getHairColors({commit, state}) {
            if (state.token) {
                return fetch("/api/haircolors/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': state.token
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.detail == "Invalid token.")
                            throw data.detail
                        commit('setHairColors', data)
                    })
                    .catch(error => {
                        console.log(error)
                        commit('setToken', null)
                    })
            }
        },
        getHandlers({commit, state}) {
            if (state.token) {
                return fetch("/api/handlers/", {
                    headers: {
                        'Accept': 'application/json',
                        'Authorization': state.token
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.detail == "Invalid token.")
                            throw data.detail
                        commit('setHandlers', data)
                    })
                    .catch(error => {
                        console.log(error)
                        commit('setToken', null)
                    })
            }
        },
    },
    getters: {
        puppets(state) {
            return state.puppets
        },
        details(state) {
            return state.details
        },
        detail: (state) => (id) => {
            return state.details[id]
        },
        issues: (state, getters) => (id) => {
            return getters.detail(id).issues
        },
        colors(state) {
            return state.colors
        },
        hair_colors(state) {
            return state.hair_colors
        },
        color: (state, getters) => (id) => {
            return getters.colors.find(color => color.id === id)
        },
        hair_color: (state, getters) => (id) => {
            return getters.hair_colors.find(hair_color => hair_color.id === id)
        },
        handlers(state) {
            return state.handlers
        },
        handler: (state, getters) => (id) => {
            return getters.handlers.find(handler => handler.id === id)
        },
        issue: (state, getters) => (puppetId, issueId) => {
            return getters.issues(puppetId).find(issue => issue.id === issueId)
        },
        token(state) {
            return state.token
        }
    }
})