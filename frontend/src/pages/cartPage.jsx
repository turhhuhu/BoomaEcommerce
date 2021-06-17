import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserCart } from "../actions/userActions";
import CartView from "../components/cartView";
import Header from "../components/header";

class CartPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserCart());
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          {this.props.isFetching ? (
            <div className="d-flex justify-content-center">
              <div className="spinner-border text-primary" role="status"></div>
            </div>
          ) : (
            <CartView />
          )}
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    isFetching: store.user.isFetching,
  };
};

export default connect(mapStateToProps)(CartPage);
