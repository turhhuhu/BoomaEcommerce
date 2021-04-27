import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/productFilter.css";

class ProductFilter extends Component {
  state = {};
  render() {
    return (
      <aside className="col-md-3">
        <div className="card">
          <article className="filter-group">
            <header className="card-header">
              <h6 className="title">Product search</h6>
            </header>
            <div className="card-body border-bottom">
              <h5 className="card-title">Search</h5>

              <div className="input-group">
                <input
                  type="text"
                  placeholder="Keyword"
                  className="form-control"
                  name=""
                ></input>
                <span className="input-group-append">
                  {" "}
                  <button className="btn btn-primary">
                    {" "}
                    <i className="fa fa-search"></i>
                  </button>
                </span>
              </div>
            </div>
            <div className="card-body">
              <h5 className="card-title">Product category</h5>
              <ul className="list-menu">
                {this.props.categories
                  ?.filter((category, index) => index < 10)
                  .map((category, index) => (
                    <li key={index}>
                      <button className="btn btn-outline-primary btn-block mt-2">
                        {" "}
                        {category}
                      </button>
                    </li>
                  ))}
              </ul>
            </div>
          </article>
        </div>
      </aside>
    );
  }
}

export default connect()(ProductFilter);
