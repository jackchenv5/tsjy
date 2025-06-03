export const variableRoutes = [
  {
    path: '/variable',
    redirect: '/variable/connector-status',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'connector-status',
        component: () => import('../views/ConnectorStatus.vue'),
      },
      {
        path: 'monitor',
        component: () => import('../views/VariableMonitor.vue'),
      },
      {
        path: 'archive',
        component: () => import('../views/VariableArchive.vue'),
      },
    ],
  },
]
