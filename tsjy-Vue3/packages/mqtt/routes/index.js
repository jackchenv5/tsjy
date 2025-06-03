export const mqttRoutes = [
  {
    path: '/mqtt',
    redirect: '/mqtt/test',
    component: () => import('#/common/layouts/DefaultLayouts.vue'),
    children: [
      {
        path: 'test',
        component: () => import('../views/MqttTest.vue'),
      },
    ],
  },
]
