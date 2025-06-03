<script setup>
import { ref, toValue } from 'vue'
import SubscriptionList from './SubscriptionList.vue'
import MessageList from './MessageList.vue'
import { lg, useResize } from '#/common/composables/resize'

defineProps(['connected'])

const loading = ref(false)

const selectedSubscription = ref('')
const handleSubscriptionSelected = (subscription) => {
  selectedSubscription.value = subscription
}

const displayAll = ref(false)
const display = ref('subscriptions')

const handleResize = (width) => {
  displayAll.value = toValue(width) >= lg
}

const { breakPoint } = useResize({ onResize: handleResize })
</script>

<template>
  <div class="mqtt-messages-root" v-loading="loading" :class="breakPoint">
    <div class="switch" v-if="!displayAll">
      <ElRadioGroup v-model="display" size="small">
        <ElRadioButton value="subscriptions" label="subscriptions">
        </ElRadioButton>
        <ElRadioButton value="messages" label="messages"></ElRadioButton>
      </ElRadioGroup>
    </div>
    <div class="container">
      <div
        class="subscription-container"
        :class="{
          display: displayAll || display === 'subscriptions',
          displayOnly: !displayAll && display === 'subscriptions',
        }"
      >
        <SubscriptionList
          :connected="connected"
          @selected="handleSubscriptionSelected"
        >
        </SubscriptionList>
      </div>
      <div
        class="messages-container"
        :class="{
          display: displayAll || display === 'messages',
        }"
      >
        <MessageList :subscription="selectedSubscription"></MessageList>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.mqtt-messages-root {
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  flex: 1;
  overflow: auto;

  > .container {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    flex: 1;
    overflow: auto;

    > .subscription-container {
      height: 100%;
      width: 30rem;
      flex-flow: column;
      grid-gap: 1rem;
      overflow: auto;
      display: none;

      &.display {
        display: flex;
      }

      &.displayOnly {
        flex: 1;
      }
    }

    > .messages-container {
      flex: 1;
      height: 100%;
      flex-flow: column;
      overflow: auto;
      display: none;

      &.display {
        display: flex;
      }
    }
  }
}
</style>
