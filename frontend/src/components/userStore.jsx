import React, { Component } from "react";
import { connect } from "react-redux";
import "../css/userStore.css";

class UserStore extends Component {
  state = {};
  render() {
    return (
      <div>
        <article className="card">
          <header class="card-header pl-2">
            <h5>Founder stores</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> owner </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> manager </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </article>

        <article className="card mt-4">
          <header class="card-header pl-2">
            <h5>Owner stores</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> owner </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> manager </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </article>

        <article className="card mt-4">
          <header class="card-header pl-2">
            <h5>Manager stores</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> owner </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> manager </td>
                  <td>
                    <a href="/home" className="btn btn-light">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </article>
      </div>
    );
  }
}

export default connect()(UserStore);
