export const shiftRoutes = [
  {
    path: '/settings',
    redirect: '/settings/shift',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'shift',
        component: () => import('../views/ShiftSetting.vue'),
      },
    ],
  },
]
