/**
 * Vuex-Store der PuppenApp.
 * Verwaltet den globalen Zustand: Liste der Puppen, Detailinformationen,
 * verfügbare Farben/Haarfarben/Handler, sowie Authentifizierungs-Token.
 */
import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export const store = new Vuex.Store({
    state: {
        puppets: [],       // Liste aller Puppen
        details: {},       // Details pro Puppe (Key = ID)
        token: null,       // Authentifizierungs-Token
        colors: [],        // Verfügbare Hautfarben
        hair_colors: [],   // Verfügbare Haarfarben
        handlers: [],      // Verfügbare Handler (Betreuer)
    },
    mutations: {
        // Setzt die komplette Puppenliste
        setPuppets(state, puppets) {
            state.puppets = puppets
        },
        // Fügt eine einzelne Puppe ins details-Objekt ein
        setPuppet(state, puppet) {
            Vue.set(state.details, puppet.id, puppet)
        },
        // Setzt die Liste der Hautfarben
        setColors(state, colors) {
            state.colors = colors
        },
        // Setzt die Liste der Haarfarben
        setHairColors(state, hair_colors) {
            state.hair_colors = hair_colors
        },
        // Setzt die Liste der Handler
        setHandlers(state, handlers) {
            state.handlers = handlers
        },
        // Setzt oder löscht den Authentifizierungs-Token (inkl. localStorage)
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
        // Lädt alle Puppen von der API
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
        // Lädt Details einer einzelnen Puppe von der API
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
        // Lädt verfügbare Hautfarben von der API
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
        // Lädt verfügbare Haarfarben von der API
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
        // Lädt verfügbare Handler von der API
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
        // Gibt die gesamte Puppenliste zurück
        puppets(state) {
            return state.puppets
        },
        // Gibt das gesamte details-Objekt zurück
        details(state) {
            return state.details
        },
        // Gibt die Details einer bestimmten Puppe per ID zurück
        detail: (state) => (id) => {
            return state.details[id]
        },
        // Gibt die Issues einer bestimmten Puppe zurück
        issues: (state, getters) => (id) => {
            return getters.detail(id).issues
        },
        // Gibt die Liste aller Hautfarben zurück
        colors(state) {
            return state.colors
        },
        // Gibt die Liste aller Haarfarben zurück
        hair_colors(state) {
            return state.hair_colors
        },
        // Gibt eine bestimmte Hautfarbe anhand der ID zurück
        color: (state, getters) => (id) => {
            return getters.colors.find(color => color.id === id)
        },
        // Gibt eine bestimmte Haarfarbe anhand der ID zurück
        hair_color: (state, getters) => (id) => {
            return getters.hair_colors.find(hair_color => hair_color.id === id)
        },
        // Gibt die Liste aller Handler zurück
        handlers(state) {
            return state.handlers
        },
        // Gibt einen bestimmten Handler anhand der ID zurück
        handler: (state, getters) => (id) => {
            return getters.handlers.find(handler => handler.id === id)
        },
        // Gibt ein bestimmtes Issue einer Puppe anhand von Puppen-ID und Issue-ID zurück
        issue: (state, getters) => (puppetId, issueId) => {
            return getters.issues(puppetId).find(issue => issue.id === issueId)
        },
        // Gibt den aktuellen Authentifizierungs-Token zurück
        token(state) {
            return state.token
        }
    }
})