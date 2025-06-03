<script setup>
import CopyrightText from '../components/CopyrightText.vue'
import AppMenu from '../components/AppMenu.vue'
import AppHeader from '../components/AppHeader.vue'
</script>

<template>
  <div class="default-layout-root">
    <aside>
      <AppMenu></AppMenu>
    </aside>
    <section class="right">
      <header>
        <AppHeader></AppHeader>
      </header>
      <main>
        <RouterView></RouterView>
      </main>
      <footer>
        <CopyrightText></CopyrightText>
      </footer>
    </section>
  </div>
</template>

<style scoped lang="scss">
.default-layout-root {
  display: flex;
  height: 100%;

  > .right {
    display: flex;
    flex-flow: column nowrap;
    flex: 1;
    width: 0;

    > main {
      flex: 1;
      background-color: var(--el-bg-color);
      overflow: auto;
      box-sizing: border-box;
      border-top: 1px solid var(--sie-divide-line-color);
      display: flex;
      flex-flow: column nowrap;
    }

    > footer {
      padding: 0.5rem 1rem;
      display: flex;
      justify-content: center;
      align-items: center;
      box-sizing: border-box;
      border-top: 1px solid var(--sie-divide-line-color);
    }
  }
}
</style>
