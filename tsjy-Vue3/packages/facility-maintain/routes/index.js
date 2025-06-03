export const facilityMaintainRoutes = [
  {
    path: '/facility-maintain',
    redirect: '/facility-maintain/part-detail',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'part-detail',
        component: () => import('../views/PartDetail.vue'),
      },
      {
        path: 'maintain-plans',
        component: () => import('../views/MaintainPlans.vue'),
      },
      {
        path: 'maintain-settings',
        component: () => import('../views/MaintainSetting.vue'),
      },
      {
        path: 'maintain-history',
        component: () => import('../views/MaintainHistory.vue'),
      },
    ],
  },
]
