export const userRoutes = [
  {
    path: '/login',
    component: () => import('../views/LoginView.vue'),
    meta: {
      anonymous: true,
    },
  },
]
