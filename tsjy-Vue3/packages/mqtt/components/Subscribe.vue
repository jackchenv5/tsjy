<script setup>
import { useDrawer } from '#/common/composables/drawer'
import { ElMessage, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { subscribeApi } from '../apis/test'

const props = defineProps(['modelValue'])
const emits = defineEmits(['update:modelValue', 'finish'])

const { showDrawer, drawerTitle, drawerSize, openDrawer, closeDrawer } =
  useDrawer()

const { t } = useLocale()

watch(
  () => props.modelValue,
  (val) => {
    if (val) {
      drawerTitle.value = t('el.mqtt.test.subscribeTopic')
      openDrawer('400')
    }
  },
)

watch(showDrawer, (val) => {
  emits('update:modelValue', val)
})

const formData = ref({
  topic: '',
  qos: 0,
})

const loading = ref(false)
const handleSubscribeTopic = async () => {
  if (loading.value) {
    return
  }

  loading.value = true
  try {
    await subscribeApi(formData.value.topic, formData.value.qos)
    ElMessage({
      message: t('el.mqtt.test.subscribeSuccessful'),
      type: 'success',
    })
    formData.value = {
      topic: '',
      qos: 0,
    }
    closeDrawer()
    emits('finish')
  } catch (e) {
    ElMessage({
      message: t('el.mqtt.test.subscribeFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <ElDrawer v-model="showDrawer" :title="drawerTitle" :size="drawerSize">
    <ElForm
      v-model="formData"
      label-position="top"
      @keydown.enter.prevent="handleSubscribeTopic"
      v-loading="loading"
    >
      <ElFormItem :label="t('el.mqtt.test.topic')">
        <ElInput v-model="formData.topic" clearable></ElInput>
      </ElFormItem>
      <ElFormItem :label="t('el.mqtt.test.qos')">
        <ElSelect v-model="formData.qos">
          <ElOption :label="t('el.mqtt.test.atMostOnce')" :value="0">
          </ElOption>
          <ElOption :label="t('el.mqtt.test.atLeastOnce')" :value="1">
          </ElOption>
          <ElOption :label="t('el.mqtt.test.exactlyOnce')" :value="2">
          </ElOption>
        </ElSelect>
      </ElFormItem>
      <ElFormItem>
        <ElButton
          type="primary"
          @click="handleSubscribeTopic"
          :loading="loading"
        >
          {{ t('el.mqtt.test.subscribe') }}
        </ElButton>
      </ElFormItem>
    </ElForm>
  </ElDrawer>
</template>

<style scoped lang="scss"></style>
