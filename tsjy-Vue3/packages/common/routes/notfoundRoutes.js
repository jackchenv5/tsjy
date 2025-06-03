export const notfoundRoutes = [
  {
    path: '/404',
    component: () => import('../layouts/DefaultLayouts.vue'),
    children: [
      {
        path: '',
        component: () => import('../views/NotFound.vue'),
      },
    ],
  },
]
