import { notfoundRoutes } from './notfoundRoutes'
import { userRoutes } from './userRoutes'
import { settingRoutes } from './settingRoutes'

export const commonRoutes = [...notfoundRoutes, ...userRoutes, ...settingRoutes]
