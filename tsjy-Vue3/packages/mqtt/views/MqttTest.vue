<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import MqttStatus from '../components/MqttStatus.vue'
import { useLocale } from 'element-plus'
import { ref } from 'vue'
import MqttMessages from '../components/MqttMessages.vue'

const { t } = useLocale()

const connected = ref(false)
const handleStatusChange = (value) => {
  connected.value = value
}
</script>

<template>
  <ContentLayout>
    <template #header>{{ t('el.mqtt.test.mqttTest') }}</template>
    <template #default>
      <div class="mqtt-test-main">
        <MqttStatus @connected="handleStatusChange"></MqttStatus>
        <MqttMessages :connected="connected"></MqttMessages>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.mqtt-test-main {
  height: 100%;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
}
</style>
