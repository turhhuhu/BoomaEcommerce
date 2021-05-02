import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserCart } from "../actions/userActions";
import CartView from "../components/cartView";
import Header from "../components/header";

class CartPage extends Component {
  state = {};

  componentDidMount(prevProps) {
    if (this.props !== prevProps) {
      this.props.dispatch(fetchUserCart(true));
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <CartView />
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(CartPage);
