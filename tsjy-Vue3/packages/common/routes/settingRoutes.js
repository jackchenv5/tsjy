export const settingRoutes = [
  {
    path: '/settings',
    redirect: '/settings/app',
    component: () => import('../layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'app',
        component: () => import('../views/AppSetting.vue'),
      },
      {
        path: 'user',
        component: () => import('../views/UserSetting.vue'),
      },
      {
        path: 'role',
        component: () => import('../views/RoleSetting.vue'),
      },
      {
        path: 'menu',
        component: () => import('../views/MenuSetting.vue'),
      },
      {
        path: 'user-profile',
        component: () => import('../views/UserProfile.vue'),
      },
    ],
  },
]
