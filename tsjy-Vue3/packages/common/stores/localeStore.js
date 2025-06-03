import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAppLocalStorage } from '../composables/appLocalStorage'
import en from 'element-plus/dist/locale/en.min.mjs'
import zhCn from 'element-plus/dist/locale/zh-cn.min.mjs'

export const useLocaleStore = defineStore('locale', () => {
  const { localStorageGetItem, localStorageSetItem } = useAppLocalStorage()

  const appEn = {
    name: en.name,
    el: {
      ...en.el,
    },
  }

  const appZhCn = {
    name: zhCn.name,
    el: {
      ...zhCn.el,
    },
  }

  const locale = ref(zhCn)

  const changeLocale = (newLocale) => {
    switch (newLocale) {
      case 'en':
        locale.value = appEn
        localStorageSetItem('lang', 'en')
        break
      default:
        locale.value = appZhCn
        localStorageSetItem('lang', 'zh-cn')
        break
    }
  }

  const addLang = (l) => {
    appEn.el = { ...appEn.el, ...l.en.el }
    appZhCn.el = { ...appZhCn.el, ...l.zhCn.el }

    const lang = localStorageGetItem('lang')
    if (lang === 'en') {
      locale.value = appEn
    } else {
      locale.value = appZhCn
    }
  }

  return { locale, changeLocale, addLang }
})
