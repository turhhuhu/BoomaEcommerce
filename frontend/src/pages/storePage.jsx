import React, { Component } from "react";
class StorePage extends Component {
  state = {};
  render() {
    console.log(this.props.match.params.guid);
    return <div></div>;
  }
}

export default StorePage;
