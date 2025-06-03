<script setup>
import Subscribe from './Subscribe.vue'
import { ElMessage, useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import { getSubscriptionsApi, unsubscribeApi } from '../apis/test'

defineProps(['connected'])
const emits = defineEmits(['selected'])

const { t } = useLocale()

const loading = ref(false)

const showSubscribeDrawer = ref(false)
const handleSubscribe = () => {
  showSubscribeDrawer.value = true
}

const handleSubscribeFinish = async () => {
  await getSubscriptions()
}

const subscriptions = ref([])
const getSubscriptions = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getSubscriptionsApi()
    subscriptions.value = res.data
    if (subscriptions.value.length) {
      if (!selectedSubscription.value) {
        selectedSubscription.value = subscriptions.value[0]
        emits('selected', selectedSubscription.value)
      }
    } else {
      emits('selected', undefined)
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await getSubscriptions()
})

const unsubscribeTopic = ref('')
const inUnsubscribe = ref(false)
const handleUnsubscribe = async (topic) => {
  if (inUnsubscribe.value) {
    return
  }

  inUnsubscribe.value = true
  unsubscribeTopic.value = topic

  try {
    await unsubscribeApi(topic)
    if (selectedSubscription.value === topic) {
      selectedSubscription.value = ''
      emits('selected', selectedSubscription.value)
    }
    ElMessage({
      message: t('el.mqtt.test.unsubscribeSuccessful'),
      type: 'success',
    })
  } catch (e) {
    ElMessage({
      message: t('el.mqtt.test.unsubscribeFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    unsubscribeTopic.value = ''
    inUnsubscribe.value = false
    await getSubscriptions()
  }
}

const selectedSubscription = ref('')
const handleSelect = (subscription) => {
  selectedSubscription.value = subscription
  emits('selected', selectedSubscription.value)
}
</script>

<template>
  <ElButton
    type="primary"
    size="small"
    plain
    @click="handleSubscribe"
    :disabled="!connected || loading || inUnsubscribe"
  >
    {{ t('el.mqtt.test.subscribeTopic') }}
  </ElButton>
  <div class="subscriptions" v-loading="loading">
    <div
      class="subscription-item"
      v-for="(subscription, index) of subscriptions"
      :key="index"
      @click="handleSelect(subscription)"
      :class="{ selected: selectedSubscription === subscription }"
    >
      <div class="content-container">
        <ElTooltip :content="subscription" :show-after="500">
          <ElText truncated>{{ subscription }}</ElText>
        </ElTooltip>
      </div>
      <ElButton
        type="danger"
        link
        @click="handleUnsubscribe(subscription)"
        :loading="inUnsubscribe && unsubscribeTopic === subscription"
      >
        {{ t('el.mqtt.test.unsubscribe') }}
      </ElButton>
    </div>
  </div>

  <Subscribe v-model="showSubscribeDrawer" @finish="handleSubscribeFinish">
  </Subscribe>
</template>

<style scoped lang="scss">
.subscriptions {
  flex: 1;
  overflow: auto;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;

  > .subscription-item {
    padding: 1rem;
    border-radius: var(--el-border-radius-base);
    display: flex;
    flex-flow: row;
    align-items: center;
    cursor: pointer;
    border: 1px solid var(--el-color-info-light-5);
    transition: all ease 0.3s;
    position: relative;
    background-color: var(--el-color-info-light-9);

    > .content-container {
      flex: 1;
      display: flex;
      flex-flow: row;
      align-items: center;
      grid-gap: 1rem;
    }

    &.selected {
      background-color: var(--el-color-primary-light-9);
      border-color: var(--el-color-primary-light-5);
    }

    &:hover {
      border-color: var(--el-color-primary);
    }
  }
}
</style>
