<script setup>
import { useSlots } from 'vue'

const footerSlot = useSlots().footer
</script>

<template>
  <section class="content-layout-root">
    <div class="card">
      <header>
        <slot name="header"></slot>
      </header>
      <main>
        <slot></slot>
      </main>
      <footer v-if="footerSlot">
        <slot name="footer"></slot>
      </footer>
    </div>
  </section>
</template>

<style scoped lang="scss">
.content-layout-root {
  padding: 1rem;
  flex: 1;
  display: flex;
  flex-flow: column nowrap;
  overflow: auto;

  > .card {
    background-color: var(--el-bg-color);
    border-radius: var(--el-border-radius-base);
    border: 1px solid var(--el-border-color);
    box-shadow: var(--el-box-shadow-light);
    flex: 1;
    display: flex;
    flex-flow: column nowrap;
    overflow: auto;

    > header {
      padding: 1rem;
      box-sizing: border-box;
      border-bottom: 1px solid var(--el-border-color);
    }

    > main {
      overflow: auto;
      flex: 1;
      padding: 1rem;
      display: flex;
      flex-flow: column nowrap;
    }

    > footer {
      padding: 1rem;
      box-sizing: border-box;
      border-top: 1px solid var(--el-border-color);
    }
  }
}
</style>
