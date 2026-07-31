/**
 * Einstiegspunkt der Vue.js-Client-App.
 * Initialisiert Vue mit Bootstrap-Vue, bindet den Vuex-Store ein
 * und setzt die Basis-URL (Produktion vs. Entwicklung).
 */
import Vue from 'vue'
import App from './App.vue'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import { store } from './util/store.js'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.config.productionTip = false
Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.config.productionTip = false
Vue.prototype.$hostname = (Vue.config.productionTip) ? 'https://akte.ruby.de' : 'http://localhost'

new Vue({
  store,
  render: h => h(App),
}).$mount('#app')

