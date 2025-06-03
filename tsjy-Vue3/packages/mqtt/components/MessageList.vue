<script setup>
import { nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { getMessagesApi } from '../apis/test'
import { dayjs, useLocale } from 'element-plus'
import { useAppLocalStorage } from '#/common/composables/appLocalStorage'

const props = defineProps(['subscription'])

const loading = ref(false)
const messages = ref()
const messageList = ref()
const autoScroll = ref(false)
const getMessages = async () => {
  if (loading.value) {
    return
  }

  loading.value = true

  try {
    const res = await getMessagesApi(props.subscription)
    messages.value = res.data
    if (autoScroll.value) {
      await nextTick(() => {
        messageList.value.scrollTop = messageList.value.scrollHeight
      })
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

let interval

watch(
  () => props.subscription,
  async (newVal) => {
    if (newVal) {
      await getMessages()
      if (interval) {
        clearInterval(interval)
      }
      interval = setInterval(async () => {
        await getMessages()
      }, 1000)
    } else {
      messages.value = []
      if (interval) {
        clearInterval(interval)
      }
    }
  },
)

const { t } = useLocale()
const qosLevel = (message) => {
  const qos = message['qos']
  return {
    'qos-0': qos === 0,
    'qos-1': qos === 1,
    'qos-2': qos === 2,
  }
}

onMounted(async () => {
  if (interval) {
    clearInterval(interval)
  }
})

onBeforeUnmount(() => {
  if (interval) {
    clearInterval(interval)
  }
})

const FORMAT_KEY = 'mqtt_message_payload_format'
const { localStorageSetItem, localStorageGetItem } = useAppLocalStorage()
const payloadFormat = ref('plaintext')

const handlePayloadFormatChange = () => {
  localStorageSetItem(FORMAT_KEY, payloadFormat.value)
}

onMounted(() => {
  payloadFormat.value = localStorageGetItem(FORMAT_KEY)
    ? localStorageGetItem(FORMAT_KEY)
    : 'plaintext'
})

const payloadFormatter = (payload) => {
  switch (payloadFormat.value) {
    case 'plaintext':
      return payload
    case 'json':
      return JSON.stringify(JSON.parse(payload), null, 2)
  }
}
</script>

<template>
  <div class="message-list-root">
    <div class="options-container">
      <ElCheckbox v-model="autoScroll" size="small">
        {{ t('el.mqtt.test.autoScroll') }}
      </ElCheckbox>
      <ElSelect
        size="small"
        style="width: 10rem"
        v-model="payloadFormat"
        @change="handlePayloadFormatChange"
      >
        <ElOption value="plaintext"></ElOption>
        <ElOption value="json"></ElOption>
      </ElSelect>
    </div>
    <div class="message-list-container" ref="messageList">
      <div
        class="message-container"
        v-for="(message, index) of messages"
        :key="index"
      >
        <div class="header-container">
          <div class="topic">
            {{ t('el.mqtt.test.topicColon') + message['topic'] }}
          </div>
          <div class="qos" :class="qosLevel(message)">
            {{ t('el.mqtt.test.qos') + ': ' + message['qos'] }}
          </div>
          <div class="retained" v-if="message['retained']">
            {{ t('el.mqtt.test.retained') }}
          </div>
        </div>
        <div class="content-container">
          <pre>{{ payloadFormatter(message['payload']) }}</pre>
        </div>
        <div class="footer-container">
          {{ dayjs.unix(message['timestamp']).format('YYYY-MM-DD HH:mm:ss') }}
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.message-list-root {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  overflow: auto;

  > .options-container {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
  }

  > .message-list-container {
    flex: 1;
    overflow: auto;
    border: 1px solid var(--el-color-primary);
    border-radius: var(--el-border-radius-base);
    padding: 1rem;
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;

    > .message-container {
      background-color: var(--el-bg-color-page);
      border-radius: var(--el-border-radius-base);
      display: flex;
      flex-flow: column;
      font-size: 1.2rem;

      > .header-container {
        display: flex;
        flex-flow: row;
        grid-gap: 1rem;
        padding: 0.5rem 1rem;
        align-items: center;

        > .topic {
          color: var(--el-text-color);
        }

        > .retained {
          background-color: var(--el-color-primary-light-5);
          border-radius: var(--el-border-radius-base);
          padding: 0 0.5rem;
        }

        > .qos {
          border-radius: var(--el-border-radius-base);
          padding: 0 0.5rem;

          &.qos-0 {
            background-color: var(--el-color-success-light-5);
          }

          &.qos-1 {
            background-color: var(--el-color-success-light-3);
          }

          &.qos-2 {
            background-color: var(--el-color-success);
          }
        }
      }

      > .content-container {
        flex: 1;
        border-top: 1px solid var(--el-border-color);
        padding: 1rem;

        > pre {
          background-color: var(--el-bg-color);
          border-radius: var(--el-border-radius-base);
          padding: 0.5rem;
          margin: 0;
        }
      }

      > .footer-container {
        padding: 0 1rem;
        background-color: var(--el-bg-color);
        color: var(--el-text-color-secondary);
      }
    }
  }
}
</style>
