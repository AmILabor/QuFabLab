<template>
  <div id="app">
    <PuppetList/>
  </div>
</template>

<script>
import PuppetList from "@/components/PuppetList";

export default {
  name: 'App',
  components: {
    PuppetList,
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  /* margin-top: 60px; */
}
</style>
