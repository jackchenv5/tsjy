export const facilityRoutes = [
  {
    path: '/settings',
    redirect: '/settings/facility',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'facility',
        component: () => import('../views/FacilitySetting.vue'),
      },
    ],
  },
]
