import { notfoundLang } from './notfound'
import { authenticationLang } from './authentication'
import { copyrightLang } from './copyright'
import { appLang } from './app'
import { userLang } from './user'
import { roleLang } from './role'
import { menuLang } from './menu'

export const commonLang = {
  en: {
    el: {
      common: {
        ...notfoundLang.en.el,
        ...authenticationLang.en.el,
        ...copyrightLang.en.el,
        ...appLang.en.el,
        ...userLang.en.el,
        ...roleLang.en.el,
        ...menuLang.en.el,
      },
    },
  },
  zhCn: {
    el: {
      common: {
        ...notfoundLang.zhCn.el,
        ...authenticationLang.zhCn.el,
        ...copyrightLang.zhCn.el,
        ...appLang.zhCn.el,
        ...userLang.zhCn.el,
        ...roleLang.zhCn.el,
        ...menuLang.zhCn.el,
      },
    },
  },
}
