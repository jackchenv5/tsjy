import { settingLang } from './setting'
import { facilityListLang } from './facilityList'

export const facilityLang = {
  en: {
    el: {
      facility: {
        ...settingLang.en.el,
        ...facilityListLang.en.el,
      },
    },
  },
  zhCn: {
    el: {
      facility: {
        ...settingLang.zhCn.el,
        ...facilityListLang.zhCn.el,
      },
    },
  },
}
