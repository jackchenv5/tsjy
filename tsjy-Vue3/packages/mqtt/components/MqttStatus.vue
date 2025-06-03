<script setup>
import { useLocale } from 'element-plus'
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { getMqttClientStatusApi } from '../apis/test'
import { Refresh } from '@element-plus/icons-vue'

const { t } = useLocale()

const loading = ref(false)
const status = ref({
  clientId: '',
  isConnected: false,
})
const emits = defineEmits(['connected'])

const getStatus = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    const res = await getMqttClientStatusApi()
    status.value = res.data
    emits('connected', res.data.isConnected)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

let intervalId
onMounted(async () => {
  await getStatus()

  if (intervalId) {
    clearInterval(intervalId)
  }
  intervalId = setInterval(async () => {
    await getStatus()
  }, 1000 * 10)
})

onBeforeUnmount(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
})
</script>

<template>
  <div class="mqtt-status-root">
    <ElTag v-if="status.isConnected" effect="dark" type="success">
      {{ t('el.mqtt.test.connected') }}
    </ElTag>
    <ElTag v-else effect="dark" type="info">
      {{ t('el.mqtt.test.disconnected') }}
    </ElTag>

    <ElButton
      size="small"
      type="primary"
      plain
      :icon="Refresh"
      :loading="loading"
      @click="getStatus"
    >
    </ElButton>
  </div>
</template>

<style scoped lang="scss">
.mqtt-status-root {
  display: flex;
  flex-flow: row;
  grid-gap: 1rem;
}
</style>
