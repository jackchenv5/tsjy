import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css'

import App from './App.vue'
import router from './router'
import common from '#/common'
import mqtt from '#/mqtt'
import variable from '#/variable'
import shift from '#/shift'
import facility from '#/facility'
import facilityStatus from '#/facility-status'

import { useLocaleStore } from '#/common/stores/localeStore.js'
import { tsjyLang } from '@/langs/index.js'

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(ElementPlus)
app.use(common)
app.use(mqtt)
app.use(variable)
app.use(shift)
app.use(facility)
app.use(facilityStatus)

app.use(() => {
  const localeStore = useLocaleStore()
  localeStore.addLang(tsjyLang)
})
app.mount('#app')
