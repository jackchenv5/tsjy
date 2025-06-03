export const facilityStatusRoutes = [
  {
    path: '/facility-status',
    redirect: '/facility-status/stopped-data',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'stopped-data',
        component: () => import('../views/StoppedData.vue'),
      },
      {
        path: 'status-binding',
        component: () => import('../views/StatusBinding.vue'),
      },
      {
        path: 'status-summary',
        component: () => import('../views/StatusSummary.vue'),
      },
    ],
  },
]
